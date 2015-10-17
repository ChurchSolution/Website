namespace Church.Website.Controllers
{
    using System;
    using System.Web.OData;

    using Church.Models;
    using Church.Website.Models;

    /// <summary>
    /// Provides an abstract controller based on ODataController.
    /// </summary>
    public class AbstractODataController : ODataController
    {
        /// <summary>
        /// The repository.
        /// </summary>
        protected readonly IRepository Repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractODataController"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository from derived classes and unit tests.
        /// </param>
        internal AbstractODataController(IRepository repository)
        {
            this.Repository = repository;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractODataController"/> class.
        /// </summary>
        protected AbstractODataController()
            : this(Church.Models.EntityFramework.Repository.Create(
                    Utilities.FrameworkEntitiesConnectionString,
                    Utilities.Configuration.ChurchWebsiteLibrary))
        {
        }

        /// <summary>
        /// Implements the dispose pattern.
        /// </summary>
        /// <param name="disposing">A value indicating whether the managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = this.Repository as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}