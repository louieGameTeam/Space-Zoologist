using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookDataModule
{
    #region Public Properties
    public NotebookConfig Config => config;
    #endregion

    #region Private Fields
    private NotebookConfig config;
    #endregion

    #region Constructors
    public NotebookDataModule(NotebookConfig config)
    {
        this.config = config;
    }
    #endregion

    #region Public Methods
    public virtual void SetConfig(NotebookConfig config)
    {
        this.config = config;
    }
    #endregion
}
