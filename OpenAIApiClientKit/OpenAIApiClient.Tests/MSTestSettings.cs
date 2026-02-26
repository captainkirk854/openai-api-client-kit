// <copyright file="MSTestSettings.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

// Default to run tests in parallel.
// [assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

// Run tests sequentially ..
// Done because tests using custom registry handlers corrupt the custom registry because of shared state.
// This is a known issue in MSTest, and there is no built-in way to isolate the state of custom registry handlers between tests.
// By setting Workers = 1, we ensure that only one test runs at a time, preventing interference between tests that use custom registry handlers.
// See: RegisterCustomHandler() in <EnsembleStrategies> and <SingleModelStrategies>.
[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.MethodLevel)]

// Workers = 1 → only one test runs at a time
// Scope = MethodLevel → MSTest will not run test methods in parallel