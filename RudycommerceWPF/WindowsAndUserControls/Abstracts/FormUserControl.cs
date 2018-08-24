using System;
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
        /// <summary>
        /// Determines whether the form is being used to update an entity
        /// </summary>
        protected bool _updatingPage = false;

        /// <summary>
        /// Defines the create event of the form
        /// When in a create form, the contentcontrol of the entity's overview is passed, so after the create, it can show the contentcontrol of entity's overview
        /// </summary>
        /// <param name="contentControl">The ContentControl of the entity's overview</param>
        public delegate void FormCreate(ContentControl contentControl);
        /// <summary>
        /// Executes the FormCreate method
        /// </summary>
        public event FormCreate CreateEvent;

        /// <summary>
        /// Defines the update event of the form
        /// </summary>
        public delegate void FormUpdate();
        /// <summary>
        /// Executes the FormUpdate method
        /// </summary>
        public event FormUpdate UpdateEvent;

        /// <summary>
        /// The ContentControl that shows the Overview of this entity. Is being used in the FormCreate event. 
        /// </summary>
        public ContentControl OverviewContentControl { get; set; }

        /// <summary>
        /// Sets the title of the title label
        /// </summary>
        protected abstract void SetTitle();

        protected void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_updatingPage)
            {
                // Update forms are shown as a popup, so on cancel it closes the window.

                Window win = (Window)Window.GetWindow(this);

                win.Close();
            }
            else
            {
                // In create forms, it has to collapse the contentControl.

                Visibility = Visibility.Collapsed;
            }            
        }

        /// <summary>
        /// Triggers the event, corresponding to required Save (whether Update or Create)
        /// </summary>
        protected void TriggerSaveEvent()
        {
            if (_updatingPage)
            {
                // Trigger the update event and close the current window
                UpdateEvent();

                var myWindow = Window.GetWindow(this);
                myWindow.Close();
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
                CreateEvent(OverviewContentControl);
            }
        }

        protected abstract void btnSave_Click(object sender, RoutedEventArgs e);
        
        /// <summary>
        /// Makes the ProgressBar start running and Disables the submit button
        /// </summary>
        protected void TurnOnProgressBar()
        {
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Visibility.Visible;

            submitButton.IsEnabled = false;
        }

        /// <summary>
        /// Defines the progressBar (needs to be defined in the child class)
        /// </summary>
        protected ProgressBar progressBar;
        /// <summary>
        /// Defines the SubmitButton (needs to be defined in the child class)
        /// </summary>
        protected Button submitButton;

        /// <summary>
        /// Makes the ProgressBar stop running and re-enables the submit button
        /// </summary>
        protected void TurnOffProgressBar()
        {
            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Visibility.Collapsed;
            submitButton.IsEnabled = true;
        }

        /// <summary>
        /// Gets the Window in which this user control exists
        /// </summary>
        /// <returns></returns>
        protected Window GetParentWindow()
        {
            Window myWindow = Window.GetWindow(this);

            return myWindow;
        }
    }
}
