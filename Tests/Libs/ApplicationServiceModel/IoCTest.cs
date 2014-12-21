﻿using MMK.Application;
using MMK.Application.ServiceLocator;
using MMK.Application.ServiceLocator.Resolving;
using NUnit.Framework;

namespace MMK.ApplicationServiceModel
{
    [TestFixture]
    public class IoCTest
    {
        [Test]
        [ExpectedException(typeof(ServiceLocatorConflictException))]
        public void ThrowsServiceLocatorNotFoundException()
        {
            IoC.Get<object>();
        }
    }

    [ServiceLocatorOwner]
    public class DummyServiceLocatorOwner
    {

        [ServiceLocator]
        public static object ServiceLocator
        {
            get
            {
                return new object();
            }
        }
    }

    [ServiceLocatorOwner]
    public class DummyServiceLocatorOwner2
    {

        [ServiceLocator]
        public static object ServiceLocator
        {
            get
            {
                return new object();
            }
        }
    }
}