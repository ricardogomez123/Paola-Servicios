﻿namespace ServicioEnviador
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller77 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller77 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller77
            // 
            this.serviceProcessInstaller77.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceInstaller77});
            this.serviceProcessInstaller77.Password = null;
            this.serviceProcessInstaller77.Username = null;
            this.serviceProcessInstaller77.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // serviceInstaller77
            // 
            this.serviceInstaller77.ServiceName = "Servicio Enviador77";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller77});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller77;
        private System.ServiceProcess.ServiceInstaller serviceInstaller77;
    }
}