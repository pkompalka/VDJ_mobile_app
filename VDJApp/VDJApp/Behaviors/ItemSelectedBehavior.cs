using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace VDJApp.Behaviors
{
    public class ItemSelectedBehavior : Behavior<ListView>
	{
		public ListView ListViewObject { get; set; }

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public IValueConverter Converter
		{
			get { return (IValueConverter)GetValue(InputConverterProperty); }
			set { SetValue(InputConverterProperty, value); }
		}

		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(ItemSelectedBehavior), null);

		public static readonly BindableProperty InputConverterProperty = BindableProperty.Create("Converter", typeof(IValueConverter), typeof(ItemSelectedBehavior), null);

		protected override void OnAttachedTo(ListView bindable)
		{
			base.OnAttachedTo(bindable);
			ListViewObject = bindable;
			bindable.BindingContextChanged += OnBindingContextChanged;
			bindable.ItemSelected += ItemSelectedMethod;
		}

		protected override void OnDetachingFrom(ListView bindable)
		{
			base.OnDetachingFrom(bindable);
			bindable.BindingContextChanged -= OnBindingContextChanged;
			bindable.ItemSelected -= ItemSelectedMethod;
			ListViewObject = null;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			BindingContext = ListViewObject.BindingContext;
		}

		private void OnBindingContextChanged(object sender, EventArgs e)
		{
			OnBindingContextChanged();
		}

		private void ItemSelectedMethod(object sender, SelectedItemChangedEventArgs e)
		{
			if (Command == null)
			{
				return;
			}

			object parameter = Converter.Convert(e, typeof(object), null, null);
			if (Command.CanExecute(parameter))
			{
				Command.Execute(parameter);
			}
		}
	}
}
