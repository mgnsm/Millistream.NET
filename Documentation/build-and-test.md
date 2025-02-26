# Build and Test

You can build the [`Millistream.NET.sln` solution](https://github.com/mgnsm/Millistream.NET/blob/master/Millistream.NET.sln) and run the tests using [Visual Studio 2022](https://visualstudio.microsoft.com/) or the [.NET Core](https://dotnet.microsoft.com/download) command-line interface (CLI):

1. [Clone](https://docs.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) or download the repository to a local folder:

        git clone https://github.com/mgnsm/Millistream.NET.git

2. Build the solution using the `dotnet build` command:

        dotnet build Millistream.NET.sln -c release

3. Run the unit tests using the `dotnet test` command:

        dotnet test Tests/Millistream.Streaming.UnitTests/Millistream.Streaming.UnitTests.csproj -c release --no-build --no-restore
        dotnet test Tests/Millistream.Streaming.DataTypes.UnitTests/Millistream.Streaming.DataTypes.UnitTests.csproj -c release --no-build --no-restore

4. Install the native API.

    To be able to run the integration tests, you first need to install the low-latency, high-throughput and high-availability C/C++ streaming API that Millistream.NET wraps. How to do this depends on your target operating system.

    On **Windows** you download and run an [.exe](https://packages.millistream.com/Windows/libmdf-1.0.29.exe). You can do this silently from a command prompt using Powershell:

        powershell (new-object System.Net.WebClient).DownloadFile('https://packages.millistream.com/Windows/libmdf-1.0.29.exe', 'libmdf-1.0.29.exe')
        .\libmdf-1.0.29.exe /S

    On **macOS** you download and install a `.pkg` file, for example in a Bash shell:

        curl -O https://packages.millistream.com/macOS/libmdf-1.0.29.pkg 
        sudo installer -pkg libmdf-1.0.29.pkg -target /

    On **Linux**, the native API and the dependent libraries are available through your distribution repository. Below is an example of how to install everything needed using the `apt-get` command-line tool on Ubuntu:

        sudo wget "https://packages.millistream.com/apt/sources.list.d/`lsb_release -cs`.list" -O /etc/apt/sources.list.d/millistream.list 
        wget -O- "https://packages.millistream.com/D2FCCE35.gpg" | gpg --dearmor | sudo tee /usr/share/keyrings/millistream-archive-keyring.gpg > /dev/null 
        sudo apt update
        sudo apt-get install libmdf

    Instructions on how to install the API on other supported distributions can be found on the [here](https://packages.millistream.com/Linux/). You may also want to take a look at the [YAML build pipeline](https://github.com/mgnsm/Millistream.NET/blob/main/.github/workflows/millistream.streaming.ci.yml) in this repository. It installs the native binaries and runs integration tests against them on macOS, Ubuntu and Windows using the cloud-hosted runners in GitHub Actions.

5. Specify valid credentials for the native API in the [Tests/Millistream.Streaming.IntegrationTests/TestSettings.runsettings](https://github.com/mgnsm/Millistream.NET/blob/master/Tests/Millistream.Streaming.IntegrationTests/TestSettings.runsettings) file:

        <RunSettings>
          <TestRunParameters>
            <Parameter name="host" value="REPLACE_THIS_VALUE" />
            <Parameter name="username" value="REPLACE_THIS_VALUE" />
            <Parameter name="password" value="REPLACE_THIS_VALUE" />
          </TestRunParameters>
        </RunSettings>

6. Run the integration tests using the `dotnet test` command:

        dotnet test Tests/Millistream.Streaming.IntegrationTests/Millistream.Streaming.IntegrationTests.csproj -c release --no-build --no-restore
            -s Tests/Millistream.Streaming.IntegrationTests/TestSettings.runsettings