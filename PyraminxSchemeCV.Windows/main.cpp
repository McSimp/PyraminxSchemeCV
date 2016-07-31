#include <iostream>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>
#include "cv.h"
#define SHOW_DEBUG 1

cv::VideoCapture* cap = nullptr;

extern "C" {
	__declspec(dllexport) void StartCapture()
	{
		cap = new cv::VideoCapture(0);
		ResetColourCounts();
	}

	__declspec(dllexport) void EndCapture()
	{
		if (cap != nullptr)
		{
			cv::destroyAllWindows();
			delete cap;
			cap = nullptr;
		}
	}

	__declspec(dllexport) void extResetColourCounts()
	{
		ResetColourCounts();
	}

	__declspec(dllexport) bool GetColours(bool mirrored, int expectedOrientation, int* colours, unsigned char* data, int outWidth, int outHeight)
	{
		cv::Mat frame;
		cap->read(frame);
		cv::imshow("camera", frame);

		cv::Mat flipped;
		cv::Mat& imToSize = frame;

		if (mirrored)
		{
			cv::flip(frame, flipped, 1);
			imToSize = flipped;
		}

		cv::Mat imsize;
		cv::resize(imToSize, imsize, cv::Size(0, 0), 0.8, 0.8);

		bool useUpright = (expectedOrientation == 0) ? true : false;

		Face face;
		bool result = ProcessFrameAndColours(frame, face, useUpright);

		std::vector<TriColour> winningColours = GetWinningColours();
		for (int i = 0; i < winningColours.size(); i++)
		{
			colours[i] = (int)winningColours[i];
		}

		// Get impolys, resize to the outwidth/outheight and send out
		cv::Mat& impolys = GetPolyMat();
		cv::Mat resizedImg(outHeight, outWidth, impolys.type());
		cv::resize(impolys, resizedImg, resizedImg.size(), cv::INTER_LINEAR);

		cv::Mat argbImg;
		cv::cvtColor(resizedImg, argbImg, CV_RGB2BGRA);
		std::vector<cv::Mat> bgra;
		cv::split(argbImg, bgra);
		std::swap(bgra[0], bgra[3]);
		std::swap(bgra[1], bgra[2]);
		std::memcpy(data, argbImg.data, argbImg.total() * argbImg.elemSize());

		return result;
	}
}
