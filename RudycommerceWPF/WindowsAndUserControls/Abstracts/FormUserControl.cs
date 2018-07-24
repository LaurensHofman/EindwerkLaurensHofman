﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    public abstract class FormUserControl: MultilingualUserControl
    {
        protected bool _updatingPage = false;

        public delegate void FormCreate(ContentControl contentControl);
        public event FormCreate CreateEvent;

        public delegate void FormUpdate();
        public event FormUpdate UpdateEvent;

        /// <summary>
        /// The current content control in which this user control exists within, 
        /// </summary>
        public ContentControl thisContentControl { get; set; }

        protected abstract void SetTitle();

        protected void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_updatingPage)
            {
                Window win = (Window)Window.GetWindow(this);

                win.Close();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }            
        }

        protected void TriggerSaveEvent()
        {
            if (_updatingPage)
            {
                UpdateEvent();

                var myWindow = Window.GetWindow(this);
                myWindow.Close();
            }
            else
            {
                CreateEvent(thisContentControl);
            }
        }

        protected abstract void btnSave_Click(object sender, RoutedEventArgs e);
        
        protected void TurnOnProgressBar()
        {
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Visibility.Visible;

            submitButton.IsEnabled = false;
        }

        protected ProgressBar progressBar;
        protected Button submitButton;

        protected void TurnOffProgressBar()
        {
            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Visibility.Collapsed;
            submitButton.IsEnabled = true;
        }
    }
}
