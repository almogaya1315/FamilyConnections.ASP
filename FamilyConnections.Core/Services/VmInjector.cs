using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Services
{
    internal class VmInjector
    {
        //private BaseApplication _app;
        //private ViewModelBrowser _browser;
        //private IRepository _Repository;
        //private CollectionFactory _collections;
        //private IChangeManager _changeManager;

        public VmInjector() //ContainerViewModel container
        {
            //_app = new BaseApplication();
            //_browser = new ViewModelBrowser(container);
            //_Repository = new Repository();
            //_collections = new CollectionFactory(_app, _Repository);
            //_changeManager = new ChangeManager();

            //_app.Users = _Repository.GetUsers();
        }

        //public ViewModelBrowser GetBrowser(ContainerViewModel container)
        //{
        //    return _browser = new ViewModelBrowser(container);
        //}

        //public T New<T>(UserModel user = null) where T : BaseViewModel, new()
        //{
        //    if (typeof(T) == typeof(MenuViewModel))
        //    {
        //        return new MenuViewModel(_browser) as T;
        //    }
        //    if (typeof(T) == typeof(ServiceViewModel))
        //    {
        //        return new ServiceViewModel(_browser) as T;
        //    }

        //    throw new InvalidOperationException();
        //}
    }
}
