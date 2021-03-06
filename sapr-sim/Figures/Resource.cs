﻿using sapr_sim.Parameters;
using sapr_sim.Parameters.Validators;
using sapr_sim.WPFCustomElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace sapr_sim.Figures
{
    [Serializable]
    public class Resource : UIEntity, ISerializable
    {

        private Rect bound;
        private Rect topExternalBound;
        private Rect bottomExternalBound;

        private Port port;

        private UIParam<Double> efficiency = new UIParam<Double>(1, new BetweenDoubleParamValidator(0.0, 1.0), "Производительность");
        private UIParam<Double> price = new UIParam<Double>(1, new BetweenDoubleParamValidator(0.0, 1000.0), "Цена");
        private UIParam<int> count = new UIParam<int>(1, new IntegerParamValidator(), "Кол-во");
        private UIParam<Boolean> isShared = new UIParam<Boolean>(true, new DefaultParamValidator(), "Разделяемый", new ParameterCheckBox(true));

        private static readonly string DEFAULT_NAME = "Ресурс";

        public Resource(Canvas canvas) : base(canvas)
        {
            init();
            textParam.Value = DEFAULT_NAME;
        }

        public Resource(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.port = info.GetValue("port", typeof(Port)) as Port;
            ports.Add(port);

            efficiency = info.GetValue("efficiency", typeof(UIParam<Double>)) as UIParam<Double>;
            price = info.GetValue("price", typeof(UIParam<Double>)) as UIParam<Double>;
            isShared = info.GetValue("isShared", typeof(UIParam<Boolean>)) as UIParam<bool>;
            isShared.ContentControl = new ParameterCheckBox(isShared.Value);
            count = info.GetValue("count", typeof(UIParam<int>)) as UIParam<int>;

            init();
        }

        public override void createAndDraw(double x, double y)
        {
            port = new Port(this, canvas, PortType.RESOURCE, x + 42.5, y - 3.5);
            canvas.Children.Add(port);
            ports.Add(port);

            label = new Label(this, canvas, x + 28, y + 22, textParam.Value);
            canvas.Children.Add(label);
        }

        public override List<UIParam> getParams()
        {
            List<UIParam> param = base.getParams();
            param.Add(efficiency);
            param.Add(price);
            param.Add(isShared);
            param.Add(count);
            return param;
        }

        public double Efficiency
        {
            get { return efficiency.Value; }
            set { efficiency.Value = value; }
        }

        public double Price
        {
            get { return price.Value; }
            set { price.Value = value; }
        }

        public Boolean IsShared
        {
            get { return isShared.Value; }
            set { isShared.Value = value; }
        }

        public int Count
        {
            get { return count.Value; }
            set { count.Value = value; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("port", port);
            info.AddValue("efficiency", efficiency);
            info.AddValue("price", price);
            info.AddValue("isShared", isShared);
            info.AddValue("count", count);

        }

        protected override System.Windows.Media.Geometry DefiningGeometry
        {
            get 
            {
                GeometryGroup gg = new GeometryGroup();
                gg.FillRule = FillRule.Nonzero;
                gg.Children.Add(new RectangleGeometry(bound));
                gg.Children.Add(new RectangleGeometry(topExternalBound));
                gg.Children.Add(new RectangleGeometry(bottomExternalBound));
                return gg;
            }
        }

        private void init()
        {
            bound = new Rect(new Size(90, 60));
            topExternalBound = new Rect(new Point(0, 0), new Point(90, 10));
            bottomExternalBound = new Rect(new Point(0, 50), new Point(90, 60));
        }
    }
}
