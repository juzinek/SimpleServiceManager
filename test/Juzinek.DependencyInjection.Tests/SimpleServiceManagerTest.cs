using Juzinek.DependencyInjection.Tests.ServiceMocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Juzinek.DependencyInjection.Tests
{
    [TestClass]
    public class SimpleServiceManagerTest
    {
        [TestMethod]
        public void InitializeTransientServiceTest()
        {
            var serviceManager = new SimpleServiceManager();
            serviceManager
                .AddTransient<IFooServiceOne, FooServiceOne>()
                .BuildServiceProvider();

            var serviceOne = serviceManager.GetRequiredService<IFooServiceOne>();
            Assert.IsNotNull(serviceOne);
            Assert.AreEqual(10, serviceOne.GetTen());
        }

        [TestMethod]
        public void InitializeSingletonServiceTest()
        {
            var serviceManager = new SimpleServiceManager();
            serviceManager
                .AddSingleton<IFooServiceOne, FooServiceOne>()
                .BuildServiceProvider();

            var serviceOne = serviceManager.GetRequiredService<IFooServiceOne>();
            Assert.IsNotNull(serviceOne);
            Assert.AreEqual(10, serviceOne.GetTen());
        }

        [TestMethod]
        public void ConfigurationAndServiceInitializationSuccessfulTest()
        {
            var serviceManager = new SimpleServiceManager();
            serviceManager
                .Configure(c => 
                {
                    c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                })
                .AddTransient<IFooServiceOne, FooServiceOne>()
                .ConfigureServices(s => 
                {
                    s.AddTransient<IFooServiceTwo, FooServiceTwo>();
                })
                .BuildServiceProvider();

            var serviceTwo = serviceManager.GetRequiredService<IFooServiceTwo>();
            Assert.IsNotNull(serviceTwo);
            Assert.AreEqual(15, serviceTwo.GetFifteen());
            Assert.AreEqual(25, serviceTwo.GetTwentyFive());
        }

        [TestMethod]
        public void GetServiceForNotConfiguredServiceReturnsNullTest()
        {
            var serviceManager = new SimpleServiceManager();
            serviceManager.BuildServiceProvider();

            var serviceTwo = serviceManager.GetService<IFooServiceTwo>();
            Assert.IsNull(serviceTwo);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void GetRequiredServiceForNotConfiguredServiceReturnsNullTest()
        {
            var serviceManager = new SimpleServiceManager();
            serviceManager.BuildServiceProvider();

            _ = serviceManager.GetRequiredService<IFooServiceTwo>();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void GetRequiredServiceBeforeContainerBuiltExpectedExceptionTest()
        {
            var serviceManager = new SimpleServiceManager();
            _ = serviceManager
                .GetRequiredService<IFooServiceOne>();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void GetServiceBeforeContainerBuiltExpectedExceptionTest()
        {
            var serviceManager = new SimpleServiceManager();
            _ = serviceManager
                .GetService<IFooServiceOne>();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ConfigureAfterContainerBuiltExpectedExceptionTest()
        {
            var serviceManager = new SimpleServiceManager();
            serviceManager.BuildServiceProvider();

            _ = serviceManager.Configure(c =>
            {
                c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            });
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ConfigureServicesAfterContainerBuiltExpectedExceptionTest()
        {
            var serviceManager = new SimpleServiceManager();
            serviceManager.BuildServiceProvider();

            _ = serviceManager.ConfigureServices(c =>
            {
                c.AddTransient<IFooServiceOne, FooServiceOne>();
            });
        }
    }
}