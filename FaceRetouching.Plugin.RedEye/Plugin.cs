using FaceRetouching.PluginSystem;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Reflection;
using Bitmap = System.Drawing.Bitmap;

namespace FaceRetouching.Plugin.RedEye;

public class Plugin : IPlugin
{
	public string Name => "Красный глаз";
	public string Description => "Убрать с изображения красный глаз";

	private void FillHoles(ref Mat mask)
	{
		var maskFloodfill = mask.Clone();
		Cv2.FloodFill(maskFloodfill, new Point(0, 0), new Scalar(255));
		var mask2 = new Mat();
		Cv2.BitwiseNot(maskFloodfill, mask2);
		mask = (mask2 | mask);
	}

	public Bitmap DoWork(Bitmap input)
	{
		var test = new Mat();
		var mat = input.ToMat();
		var matOut = mat.Clone();

		var eyeCascade = new CascadeClassifier(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/haarcascade_eye.xml");
		var eyes = eyeCascade.DetectMultiScale(mat, 1.3, 4, 0 | HaarDetectionTypes.ScaleImage, new(100, 100)).ToList();

		eyes.ForEach(eyeRect =>
		{
			var eye = new Mat(mat, eyeRect);

			var bgr = eye.Split();

			var mask = new Mat();
			mask = mask.BitwiseAnd(bgr[2].GreaterThan(150).ToMat() & bgr[2].GreaterThan(bgr[1] + bgr[0]).ToMat()).ToMat();

			FillHoles(ref mask);
			Cv2.Dilate(mask, mask, null, new Point(-1, -1), 3, BorderTypes.Replicate, new Scalar(1));

			var mean = (bgr[0] + bgr[1]) / 2;
			Cv2.CopyTo(bgr[2], mask, mean);
			Cv2.CopyTo(bgr[0], mask, mean);
			Cv2.CopyTo(bgr[1], mask, mean);

			var eyeOut = new Mat();
			Cv2.Merge(bgr, eyeOut);

			eyeOut.CopyTo(new Mat(matOut, eyeRect));
		});


		return matOut.ToBitmap();
	}
}
