﻿/////////////////////////////////////////////////////////////////////////////////////////////////
//
// FSharp.Control.FusionTasks - F# Async computation <--> .NET Task easy seamless interoperability library.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AsyncExtensions =

  ///////////////////////////////////////////////////////////////////////////////////
  // F# side Async class extensions.

  type Async with

    /// <summary>
    /// Seamless conversion from F# Async to .NET Task.
    /// </summary>
    /// <param name="async">F# Async</param>
    /// <param name="token">Cancellation token (optional)</param>
    /// <returns>.NET Task</returns>
    static member AsTask(async: Async<unit>, ?token: CancellationToken) =
      Infrastructures.asTask(async, token) :> Task

    /// <summary>
    /// Seamless conversion from F# Async to .NET Task.
    /// </summary>
    /// <typeparam name="'T">Computation result type</typeparam> 
    /// <param name="async">F# Async</param>
    /// <param name="token">Cancellation token (optional)</param>
    /// <returns>.NET Task</returns>
    static member AsTask(async: Async<'T>, ?token: CancellationToken) =
      Infrastructures.asTask(async, token)

  ///////////////////////////////////////////////////////////////////////////////////
  // F# side Task class extensions.

  type Task with
  
    /// <summary>
    /// Seamless conversion from .NET Task to F# Async.
    /// </summary>
    /// <param name="task">.NET Task</param>
    /// <param name="token">Cancellation token (optional)</param>
    /// <returns>F# Async</returns>
    member task.AsAsync(?token: CancellationToken) =
      Infrastructures.asAsync(task, token)

    /// <summary>
    /// Seamless conversionable substitution Task.ConfigureAwait()
    /// </summary>
    /// <param name="task">.NET Task</param>
    /// <param name="continueOnCapturedContext">True if continuation running on captured SynchronizationContext</param>
    /// <returns>ConfiguredAsyncAwaitable</returns>
    member task.AsyncConfigure(continueOnCapturedContext: bool) =
      ConfiguredAsyncAwaitable(task.ConfigureAwait(continueOnCapturedContext))

  type Task<'T> with

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async.
    /// </summary>
    /// <typeparam name="'T">Computation result type</typeparam> 
    /// <param name="task">.NET Task</param>
    /// <param name="token">Cancellation token (optional)</param>
    /// <returns>F# Async</returns>
    member task.AsAsync(?token: CancellationToken) =
      Infrastructures.asAsyncT(task, token)

    /// <summary>
    /// Seamless conversionable substitution Task.ConfigureAwait()
    /// </summary>
    /// <param name="task">.NET Task&lt;'T&gt;</param>
    /// <param name="continueOnCapturedContext">True if continuation running on captured SynchronizationContext</param>
    /// <returns>ConfiguredAsyncAwaitable</returns>
    member task.AsyncConfigure(continueOnCapturedContext: bool) =
      ConfiguredAsyncAwaitable<'T>(task.ConfigureAwait(continueOnCapturedContext))
  
  ///////////////////////////////////////////////////////////////////////////////////
  // F# side ConfiguredAsyncAwaitable class extensions.

  type ConfiguredAsyncAwaitable with
  
    /// <summary>
    /// Seamless conversion from .NET Task to F# Async.
    /// </summary>
    /// <param name="cta">.NET ConfiguredTaskAwaitable (expr.ConfigureAwait(...))</param>
    /// <returns>F# Async</returns>
    member cta.AsAsync() =
      Infrastructures.asAsyncCTA(cta)
 
  type ConfiguredAsyncAwaitable<'T> with

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async.
    /// </summary>
    /// <typeparam name="'T">Computation result type</typeparam> 
    /// <param name="cta">.NET ConfiguredTaskAwaitable (expr.ConfigureAwait(...))</param>
    /// <returns>F# Async</returns>
    member cta.AsAsync() =
      Infrastructures.asAsyncCTAT(cta)
  
  ///////////////////////////////////////////////////////////////////////////////////
  // F# side async computation builder extensions.

  type AsyncBuilder with

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async in Async workflow.
    /// </summary>
    /// <param name="expr">.NET Task (expression result)</param>
    /// <returns>F# Async</returns>
    member __.Source(expr: Task) =
      Infrastructures.asAsync(expr, None)

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async in Async workflow.
    /// </summary>
    /// <typeparam name="'T">Computation result type</typeparam> 
    /// <param name="expr">.NET Task (expression result)</param>
    /// <returns>F# Async</returns>
    member __.Source(expr: Task<'T>) =
      Infrastructures.asAsyncT(expr, None)

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async in Async workflow.
    /// </summary>
    /// <param name="cta">.NET ConfiguredTaskAwaitable (expr.ConfigureAwait(...))</param>
    /// <returns>F# Async</returns>
    member __.Source(cta: ConfiguredAsyncAwaitable) =
      Infrastructures.asAsyncCTA(cta)

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async in Async workflow.
    /// </summary>
    /// <typeparam name="'T">Computation result type</typeparam> 
    /// <param name="cta">.NET ConfiguredTaskAwaitable (expr.ConfigureAwait(...))</param>
    /// <returns>F# Async</returns>
    member __.Source(cta: ConfiguredAsyncAwaitable<'T>) =
      Infrastructures.asAsyncCTAT(cta)

  ///////////////////////////////////////////////////////////////////////////////////
  // F# side synchronizer extensions.

  type AsyncLock with

    /// <summary>
    /// Try asyncronos lock. (For F# native)
    /// </summary>
    /// <returns>
    /// Async computation.
    /// </returns>
    member this.AsyncLock() =
      this.asyncLock()
    
    /// <summary>
    /// Try asyncronos lock. (For F# native)
    /// </summary>
    /// <param name="token">Cancellation token (optional)</param>
    /// <returns>
    /// Async computation.
    /// </returns>
    member this.AsyncLock(token: CancellationToken) =
      this.asyncLock(token)

  type AsyncLazy<'T> with

    /// <summary>
    /// Get or generate instance asynchronosly.
    /// </summary>
    /// <returns>
    /// 'T instance.
    /// </returns>
    member this.AsyncGetValue() =
      this.asyncGetValue()
