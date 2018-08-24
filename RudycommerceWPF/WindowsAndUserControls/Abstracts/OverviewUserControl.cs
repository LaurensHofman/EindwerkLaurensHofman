using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    public abstract class OverviewUserControl: MultilingualUserControl
    {
        /// <summary>
        /// Creates instance of the Generic Form type, with ID to load the Item in the form
        /// </summary>
        protected void ShowUpdateForm<FormUC>(int ID) where FormUC: FormUserControl, new()
        {
            // https://stackoverflow.com/questions/1852837/is-there-a-generic-constructor-with-parameter-constraint-in-c

            // Creates an instance of the Generic FormUC, using the constructor that accepts an integer as parameter.
            // Because C# cannot know yet that the UserControls that inherit from FormUC will be able to accept an int as parameter,
            // I had to create an instance in another than the usual way.

            FormUC form = (FormUC)Activator.CreateInstance(typeof(FormUC), (int)ID);

            // Calls the Updated method when the update event is triggered in the form

            form.UpdateEvent += Updated;

            // Shows a popup window with the new form loaded.

            UpdateFormWindow win = new UpdateFormWindow(form);
            win.Show();
        }

        public abstract Task LoadDataGridData();

        /// <summary>
        /// To be called when the Update event gets triggered in an update form, opened from within this Overview
        /// </summary>
        protected virtual void Updated()
        {
            LoadDataGridData();
        }

        /// <summary>
        /// Gets the Window in which this user control exists
        /// </summary>
        /// <returns></returns>
        protected Window GetParentWindow()
        {
            NavigationWindow myWindow = (NavigationWindow)Window.GetWindow(this);

            return myWindow;
        }

        protected void RefreshGrid(object sender, RoutedEventArgs e)
        {
            LoadDataGridData();
        }

        protected abstract void Delete(object sender, RoutedEventArgs e);

        protected abstract void Update(object sender, RoutedEventArgs e);

        protected abstract void OpenForm(object sender, RoutedEventArgs e);
    }
}
