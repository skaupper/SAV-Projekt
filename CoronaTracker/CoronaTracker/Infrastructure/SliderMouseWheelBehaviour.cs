﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CoronaTracker.Infrastructure
{
    // Source: https://stackoverflow.com/questions/42294278/how-to-change-a-sliders-value-with-the-mouse-wheel
    class SliderMouseWheelBehaviour
    {
        public static double GetValue(Slider slider)
        {
            return (double)slider.GetValue(ValueProperty);
        }

        public static void SetValue(Slider slider, double value)
        {
            slider.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached(
            "Value",
            typeof(double),
            typeof(SliderMouseWheelBehaviour),
            new UIPropertyMetadata(0.0, OnValueChanged));

        public static Slider GetSlider(UIElement parentElement)
        {
            return (Slider)parentElement.GetValue(SliderProperty);
        }

        public static void SetSlider(UIElement parentElement, Slider value)
        {
            parentElement.SetValue(SliderProperty, value);
        }

        public static readonly DependencyProperty SliderProperty =
            DependencyProperty.RegisterAttached(
            "Slider",
            typeof(Slider),
            typeof(SliderMouseWheelBehaviour));


        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider slider = d as Slider;
            slider.Loaded += (ss, ee) =>
            {
                Window window = Window.GetWindow(slider);
                if (window != null)
                {
                    SetSlider(window, slider);
                    window.PreviewMouseWheel += Window_PreviewMouseWheel;
                }
            };
            slider.Unloaded += (ss, ee) =>
            {
                Window window = Window.GetWindow(slider);
                if (window != null)
                {
                    SetSlider(window, null);
                    window.PreviewMouseWheel -= Window_PreviewMouseWheel;
                }
            };
        }

        private static void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Window window = sender as Window;
            Slider slider = GetSlider(window);
            double value = GetValue(slider);
            if (slider != null && value != 0)
            {
                double tmpSliderValue = slider.Value;
                tmpSliderValue += slider.SmallChange * e.Delta / value;

                if (tmpSliderValue < slider.Minimum)
                    slider.Value = slider.Minimum;
                else if (tmpSliderValue > slider.Maximum)
                    slider.Value = slider.Maximum;
                else
                    slider.Value = tmpSliderValue;
            }
        }
    }
}
