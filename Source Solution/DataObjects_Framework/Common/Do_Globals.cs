﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects_Framework.Common
{
    /// <summary>
    /// All the framework wide variables can be accessed here.
    /// </summary>
    public static class Do_Globals
    {
        static Do_Globals() { }

        /// <summary>
        /// Global Settings for the framework can be set here.
        /// </summary>
        public static Global_Settings gSettings
        {
            get { return Global_Settings.Instance; }
        }
    }

    /// <summary>
    /// Global Settings class (Singleton)
    /// </summary>
    public sealed class Global_Settings
    {
        #region _Variables

        Dictionary<string, object> mCollection = new Dictionary<string, object>();

        #endregion

        #region _Constructor

        static readonly Global_Settings mInstance =
            new Global_Settings()
            {
                pConnectionString = "",
                pUseSoftDelete = false,
                pDataAccessType = Do_Constants.eDataAccessType.DataAccess_SqlServer
            };

        static Global_Settings() { }

        #endregion

        #region _Properties

        /// <summary>
        /// Get instance object
        /// </summary>
        public static Global_Settings Instance
        {
            get { return mInstance; }
        }

        /// <summary>
        /// Sets the connection string the framework will use during fetching
        /// </summary>
        public string pConnectionString { get; set; }

        public string pWcfAddress { get; set; }

        /// <summary>
        /// Sets if the framework will use the soft delete feature, 
        /// will require the "IsDeleted As Bit" field on tables to use this feature.
        /// </summary>
        public bool pUseSoftDelete { get; set; }

        public Do_Constants.eDataAccessType pDataAccessType { get; set; }

        /// <summary>
        /// Generic Dictionary of settings, fill it up as needed.
        /// </summary>
        public Dictionary<string, object> pCollection
        {
            get { return this.mCollection; }
        }

        #endregion
    }

}
