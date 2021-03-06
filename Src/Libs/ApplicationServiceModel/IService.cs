﻿namespace MMK.ApplicationServiceModel
{
    public interface IService
    {
        bool IsInitialized { get; }

        void Initialize();

        void Start();
        void Stop();
    }
}