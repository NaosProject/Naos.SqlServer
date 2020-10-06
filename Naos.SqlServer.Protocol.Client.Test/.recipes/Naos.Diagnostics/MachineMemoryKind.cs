﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MachineMemoryKind.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Naos.Recipes source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Diagnostics.Recipes
{
    /// <summary>
    /// Specifies the kind of machine memory.
    /// </summary>
#if NaosDiagnosticsRecipes
    public
#else
    [System.CodeDom.Compiler.GeneratedCode("Naos.Diagnostics", "See package version number")]
    internal
#endif
    enum MachineMemoryKind
    {
        /// <summary>
        /// The total physical memory.
        /// </summary>
        TotalPhysical,

        /// <summary>
        /// The total virtual memory.
        /// </summary>
        TotalVirtual,

        /// <summary>
        /// The available physical memory.
        /// </summary>
        AvailablePhysical,

        /// <summary>
        /// The avilable virtual memory.
        /// </summary>
        AvailableVirtual,
    }
}
