# Continuous Integration and Delivery (CI/CD)
Millistream.NET is continuously integrated using GitHub Actions. Each pull-request (PR) against the protected `main` branch is automatically validated (the source code is built and tested) on cloud-hosted runners for macOS, Linux and Windows using the .NET Core CLI.

There is a YAML-based CD pipeline (or action) for each package ([Millistream.Streaming](https://www.nuget.org/packages/Millistream.Streaming) and [Millistream.Streaming.DataTypes](https://www.nuget.org/packages/Millistream.Streaming.DataTypes)) that builds and validates the `main` branch, versions the assemblies and produces a NuGet package:

- [Millistream.Streaming CD](../tree/main/.github/workflows/millistream.streaming.cd.yml)
- [Millistream.Streaming.DataTypes CD](../tree/main/.github/workflows/millistream.streaming.datatypes.cd.yml)

They are triggered when the repository is [tagged](https://git-scm.com/book/en/v2/Git-Basics-Tagging) with a release version.

Separate CI pipelines/actions are used to validate the PRs:

- [Millistream.Streaming CI](../tree/main/.github/workflows/millistream.streaming.ci.yml)
- [Millistream.Streaming.DataTypes CI](../tree/main/.github/workflows/millistream.streaming.datatypes.ci.yml)

## Deployment
The packages that are produced by the CD actions are currently being manually validated and uploaded to NuGet.