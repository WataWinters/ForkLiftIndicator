using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indicator
{

    public class MainViewModel : INotifyPropertyChanged
    {

        private string _back_ground_src;

        public string back_ground_src
        {
            get { return _back_ground_src; }

            set
            {
                if (_back_ground_src != value)
                {
                    _back_ground_src = value;
                    OnPropertyChanged(nameof(back_ground_src));
                }
            }
        }



        private string _QR_Label;

        public string QR_Label
        {
            get { return _QR_Label; }

            set
            {
                if (_QR_Label != value)
                {
                    _QR_Label = value;
                    OnPropertyChanged(nameof(QR_Label));
                }
            }
        }

        private string _TotalWeight_Label;
        public string TotalWeight_Label
        {
            get { return _TotalWeight_Label; }

            set
            {
                if (_TotalWeight_Label != value)
                {
                    _TotalWeight_Label = value;
                    OnPropertyChanged(nameof(TotalWeight_Label));
                }
            }
        }

        private string _RightStatusLabel;
        public string RightStatusLabel
        {
            get { return _RightStatusLabel; }

            set
            {
                if (_RightStatusLabel != value)
                {
                    _RightStatusLabel = value;
                    OnPropertyChanged(nameof(RightStatusLabel));
                }
            }
        }

        private string _LeftStatusLabel;
        public string LeftStatusLabel
        {
            get { return _LeftStatusLabel; }

            set
            {
                if (_LeftStatusLabel != value)
                {
                    _LeftStatusLabel = value;
                    OnPropertyChanged(nameof(LeftStatusLabel));
                }
            }
        }


        private Color _RightColorLabel;
        public Color RightColorLabel
        {
            get { return _RightColorLabel; }

            set
            {
                if (_RightColorLabel != value)
                {
                    _RightColorLabel = value;
                    OnPropertyChanged(nameof(RightColorLabel));
                }
            }
        }

        private Color _LeftColorLabel;
        public Color LeftColorLabel
        {
            get { return _LeftColorLabel; }

            set
            {
                if (_LeftColorLabel != value)
                {
                    _LeftColorLabel = value;
                    OnPropertyChanged(nameof(LeftColorLabel));
                }
            }
        }




        private string _RightWeightLabel;
        public string RightWeightLabel
        {
            get { return _RightWeightLabel; }

            set
            {
                if (_RightWeightLabel != value)
                {
                    _RightWeightLabel = value;
                    OnPropertyChanged(nameof(RightWeightLabel));
                }
            }
        }

        private string _LeftWeightLabel;
        public string LeftWeightLabel
        {
            get { return _LeftWeightLabel; }

            set
            {
                if (_LeftWeightLabel != value)
                {
                    _LeftWeightLabel = value;
                    OnPropertyChanged(nameof(LeftWeightLabel));
                }
            }
        }

        private string _Vision_h_Label;
        public string Vision_h_Label
        {
            get { return _Vision_h_Label; }

            set
            {
                if (_Vision_h_Label != value)
                {
                    _Vision_h_Label = value;
                    OnPropertyChanged(nameof(Vision_h_Label));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
