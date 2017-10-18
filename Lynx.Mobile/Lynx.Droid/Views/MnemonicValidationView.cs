using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform.Core;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Lynx.Core.Interactions;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for MainViewModel")]
    class MnemonicValidationView : MvxActivity
    {
        private IMvxInteraction<MnemonicValidationInteraction> _createButtons;
        private List<Button> _validationButtons = new List<Button>();

        public IMvxInteraction<MnemonicValidationInteraction> CreateButtons
        {
            get => _createButtons;
            set
            {
                if (_createButtons != null)
                    _createButtons.Requested -= CreateMnemonicValidationButtons;

                _createButtons = value;
                _createButtons.Requested += CreateMnemonicValidationButtons;
            }
        }

        private void CreateMnemonicValidationButtons(object sender, MvxValueEventArgs<MnemonicValidationInteraction> e)
        {
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.mnemonicValidationView);

            //Clear out the buttons 
            if(_validationButtons.Count != 0)
                foreach(Button btn in _validationButtons)
                    layout.RemoveView(btn);

            if (e.Value.buttons == null) return;

            foreach (string word in e.Value.buttons)
            {
                Button btn = new Button(this);
                btn.Text = word;
                btn.Click += (o, a) => e.Value.onButtonClick(word);
                _validationButtons.Add(btn);
                layout.AddView(btn);
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MnemonicValidationView);
            CreateBindings();
        }


        private void CreateBindings()
        {
            var set = this.CreateBindingSet<MnemonicValidationView, MnemonicValidationViewModel>();
            set.Bind(this).For(view => view.CreateButtons).To(viewModel => viewModel.CreateButtons).OneWay();
            set.Apply();
        }
    }
}