// -----------------------------------------------------------------------
// <copyright file="AuthorizedController.cs" company="WiseDonation.com">
// Copyright (c) 2011 All Right Reserved, https://wisedonation.com/
// -----------------------------------------------------------------------

using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace eZeeFlow.Web.Controllers
{
    /// <summary>
    /// Provides base for controllers that need authorization and user information.
    /// </summary>
    /// <remarks>
    /// This base controller largely provides common methods to recover authorized user information.
    /// </remarks>
    public class AuthorizedController : Controller
    {
        //protected readonly IUserServices UserServices;
        private readonly IServiceLocator serviceLocator;

       
        /// <summary>
        /// Generic using<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T Using<T>() where T : class
        {
            var handler = serviceLocator.GetInstance<T>();
            if (handler == null)
            {
                throw new NullReferenceException("Unable to resolve type with service locator; type " + typeof(T).Name);
            }
            return handler;
        }
    }
}