using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace VDJApp.Behaviors
{
    public class OnAppearingBehavior : Behavior<ContentPage>
    {
        public ContentPage ContentPageObject { get; private set; }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(OnAppearingBehavior), null);

        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            ContentPageObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            bindable.Appearing += OnAppearingMethod;
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            bindable.Appearing -= OnAppearingMethod;
            ContentPageObject = null;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = ContentPageObject.BindingContext;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        private void OnAppearingMethod(object sender, object e)
        {
            if (Command == null)
            {
                return;
            }

            object parameter;
            parameter = e;

            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }
    }
}
