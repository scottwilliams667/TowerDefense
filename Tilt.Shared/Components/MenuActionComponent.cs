using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Components
{
    public abstract class ActionComponent : InputComponent
    {
        protected ActionComponent(Entity owner) : base(owner)
        {
        }

        public abstract void Execute();
    }

    public class MenuActionComponent : ActionComponent
    {
        private Action mAction;

        public MenuActionComponent(Action menuAction, Entity owner) : base(owner)
        {
            mAction = menuAction;
        }

        public Action Action
        {
            get { return mAction; }
            set { mAction = value; }
        }

        public override void Execute()
        {
            if(mAction != null)
                mAction();
        }
    }

    public class MenuActionArgComponent : ActionComponent
    {
        private Action<object> mAction1;
        private Action<object, object> mAction2;
        private Action<object, object, object> mAction3;
        private Action<object,object,object,object> mAction4;
        private object mObj1;
        private object mObj2;
        private object mObj3;
        private object mObj4;

        public MenuActionArgComponent(Action<object> menuAction, object obj, Entity owner) : base(owner)
        {
            mAction1 = menuAction;
            mObj1 = obj;
        }

        public MenuActionArgComponent(Action<object, object> menuAction, object obj1, object obj2, Entity owner)
            : base(owner)
        {
            mAction2 = menuAction;
            mObj1 = obj1;
            mObj2 = obj2;
        }

        public MenuActionArgComponent(Action<object, object, object> menuAction, object obj1, object obj2, object obj3, Entity owner)
            : base(owner)
        {
            mAction3 = menuAction;
            mObj1 = obj1;
            mObj2 = obj2;
            mObj3 = obj3;
        }

        public MenuActionArgComponent(Action<object, object, object, object> menuAction, object obj1, object obj2, object obj3, object obj4, Entity owner)
            : base(owner)
        {
            mAction4 = menuAction;
            mObj1 = obj1;
            mObj2 = obj2;
            mObj3 = obj3;
            mObj4 = obj4;
        }

        public override void Execute()
        {
            if (mAction4 != null)
                mAction4(mObj1, mObj2, mObj3, mObj4);
            if (mAction3 != null)
                mAction3(mObj1, mObj2, mObj3);
            if (mAction2 != null)
                mAction2(mObj1, mObj2);
            if (mAction1 != null)
                mAction1(mObj1);
        }

       

    }
}
