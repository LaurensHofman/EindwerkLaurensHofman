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

            FormUC form = (FormUC)Activator.CreateInstance(typeof(FormUC), (int)ID);

            form.UpdateEvent += Updated;

            UpdateFormWindow win = new UpdateFormWindow(form);
            win.Show();
        }

        public abstract Task LoadDataGridData();

        /// <summary>
        /// To be called when the Update event gets triggered in an update form, opened from within this Overview
        /// </summary>
        private void Updated()
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
