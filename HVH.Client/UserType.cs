/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */
 
namespace HVH.Client
{
    /// <summary>
    /// The access levels of an account
    /// </summary>
    public enum UserType
    {
        Teacher = 512,
        Normal = 256,
        Admin = 1024
    }
}