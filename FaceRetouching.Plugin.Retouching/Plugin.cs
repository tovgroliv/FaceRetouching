using FaceRetouching.PluginSystem;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Face;
using System.Collections.Immutable;
using Bitmap = System.Drawing.Bitmap;
using System.Reflection;

#pragma warning disable CA1416

namespace FaceRetouching.Plugin.Retouching;

public class Plugin : IPlugin
{
	public string Name => "Ретуширование лица";
	public string Description => "Ретуширование лица";

	public Bitmap DoWork(Bitmap input)
	{
		var mat = input.ToMat();
		var gray = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
		var matOut = mat.Clone();

		var faceCascade = new CascadeClassifier(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/haarcascade_frontalface_default.xml");
		var faces = faceCascade.DetectMultiScale(gray, 1.1, 5, minSize: new(40, 40)).ToList();

		var facemark = FacemarkLBF.Create();
		facemark.LoadModel(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/lbfmodel.yaml");

		if (faces.Count > 0 && facemark.Fit(mat, InputArray.Create(faces.ToArray()), out var landmarks))
		{
			var faceId = 0;

			landmarks.ToList().ForEach(faceLandmarks =>
			{
				var faceRect = faces[faceId++];

				var leftEye = faceLandmarks.Skip(36).Take(6).Select(x => new Point(x.X, x.Y));
				var leftEyebrow = faceLandmarks.Skip(17).Take(5).Select(x => new Point(x.X, x.Y));

				var rightEye = faceLandmarks.Skip(42).Take(6).Select(x => new Point(x.X, x.Y));
				var rightEyebrow = faceLandmarks.Skip(22).Take(5).Select(x => new Point(x.X, x.Y));

				var mouth = faceLandmarks.Skip(48).Take(12).Select(x => new Point(x.X, x.Y));

				mat.FillPoly([leftEye, leftEyebrow, rightEye, rightEyebrow, mouth], new(255, 255, 255));

				faceLandmarks.ToList().ForEach(landmark =>
				{
					mat.Circle((int)(landmark.X - 1), (int)(landmark.Y - 1), 2, new Scalar(0, 0, 255), thickness: 2);
					mat.Circle((int)(landmark.X - 2), (int)(landmark.Y - 2), 4, new Scalar(255, 255, 0), thickness: 1);
				});

				var onlyFace = mat.Clone(faceRect);
				onlyFace.CvtColor(ColorConversionCodes.BGR2YCrCb);

				var ycrcb = onlyFace.Split();

				// мю - среднее значени
				var means = new double[3];
				// сигма - стандартное отклонение
				var stdDeviations = new double[3];
				for (int i = 0; i < 3; i++)
				{
					means[i] = Cv2.Mean(ycrcb[i])[0];
					Cv2.MeanStdDev(ycrcb[i], out _, out Scalar stdDev);
					stdDeviations[i] = stdDev.Val0;
				}

				for (int i = 0; i < 3; i++)
				{
					ycrcb[i].Get
				}
			});
		}

		faces.ForEach(faceRect =>
		{
			var face = new Mat(mat, faceRect);

			mat.Rectangle(faceRect, new Scalar(0, 255, 0), thickness: 4);
		});

		matOut = mat.Clone();

		return matOut.ToBitmap();
	}
}
