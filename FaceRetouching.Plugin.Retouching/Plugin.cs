using FaceRetouching.PluginSystem;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Face;
using System.Collections.Immutable;
using Bitmap = System.Drawing.Bitmap;
using System.Reflection;
using System.Threading.Tasks;


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

				var face = faceLandmarks.Skip(0).Take(17).Select(x => new Point(x.X, x.Y)).ToList();

				var last = face.Last();
				last.X = faceRect.X + faceRect.Width;
				face.Add(last);
				face.Add(new(faceRect.Width + faceRect.X, faceRect.Height + faceRect.Y));
				face.Add(new(faceRect.X, faceRect.Height + faceRect.Y));
				var first = face.First();
				first.X = faceRect.X;
				face.Add(first);

				//faceLandmarks.ToList().ForEach(landmark =>
				//{
				//	mat.Circle((int)(landmark.X - 1), (int)(landmark.Y - 1), 2, new Scalar(0, 0, 255), thickness: 2);
				//	mat.Circle((int)(landmark.X - 2), (int)(landmark.Y - 2), 4, new Scalar(255, 255, 0), thickness: 1);
				//});

				var onlyFace = mat.Clone(faceRect);
				onlyFace.CvtColor(ColorConversionCodes.BGR2YCrCb);

				var ycrcb = onlyFace.Split();

				mat.FillPoly([leftEye, leftEyebrow, rightEye, rightEyebrow, mouth, face], new(255, 255, 255));
				onlyFace = mat.Clone(faceRect);

				// мю - среднее значени
				var means = new double[3];
				// сигма - стандартное отклонение
				var stdDeviations = new double[3];
				for (int i = 0; i < 3; i++)
				{
					Cv2.MeanStdDev(ycrcb[i], out var mean, out var stdDeviation);
					means[i] = mean[0];
					stdDeviations[i] = stdDeviation[0];
				}

				var mask = onlyFace.Clone();

				for (int y = 0; y < mask.Height; y++)
				{
					for (int x = 0; x < mask.Width; x++)
					{
						var pixel = onlyFace.Get<Vec3b>(x, y);

						var yw = pixel[0];
						var crw = pixel[1];
						var cbw = pixel[2];

						Vec3b value = 
							means[0] - 2 * stdDeviations[0] <= yw  && yw  <= means[0] + 2 * stdDeviations[0] &&
							means[1] - 2 * stdDeviations[1] <= crw && crw <= means[1] + 2 * stdDeviations[1] &&
							means[2] - 2 * stdDeviations[2] <= cbw && cbw <= means[2] + 2 * stdDeviations[2]
							? new(255,255,255) : new(0, 0, 0);

						mask.Set<Vec3b>(x, y, value);
					}
				}

				for (int y = 0; y < mask.Height; y++)
				{
					for (int x = 0; x < mask.Width; x++)
					{
						var pixel = onlyFace.Get<Vec3b>(x, y);

						if (mask.Get<Vec3b>(x, y) == new Vec3b(0, 0, 0))
						{
							onlyFace.Set<Vec3b>(x, y, new(0, 0, 0));
						}
					}
				}


				Mat blurredImage = onlyFace.Clone();

				// Применение фильтра размытия по Гауссу с небольшим радиусом
				Cv2.GaussianBlur(blurredImage, blurredImage, new Size(5, 5), 1.5);

				// Преобразование изображений в формат Lab для удобства доступа к яркостной составляющей (L)
				Mat labOriginal = new Mat();
				Mat labBlurred = new Mat();
				Cv2.CvtColor(onlyFace, labOriginal, ColorConversionCodes.BGR2Lab);
				Cv2.CvtColor(blurredImage, labBlurred, ColorConversionCodes.BGR2Lab);

				// Вычитание почленно яркостной составляющей каждого пикселя оригинального изображения от размытого изображения
				Mat resultImage = new Mat();
				Cv2.Subtract(labBlurred.Split()[0], labOriginal.Split()[0], resultImage);

				// Выравнивание тона с помощью билатерального размытия
				Mat toneMappedImage = new Mat();
				Cv2.BilateralFilter(onlyFace, toneMappedImage, 15, 80, 80);

				mat = toneMappedImage.Clone();
			});
		}

		//faces.ForEach(faceRect =>
		//{
		//	var face = new Mat(mat, faceRect);

		//	mat.Rectangle(faceRect, new Scalar(0, 255, 0), thickness: 4);
		//});

		matOut = mat.Clone();

		return matOut.ToBitmap();
	}
}
