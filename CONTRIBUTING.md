# Contributing to the Traffic 3D project

## Technologies

This project is based on the [Unity 3d games engine](https://unity3d.com/unity).
The AI is written in [Python3](https://www.python.org/) with [PyTorch](https://pytorch.org/).

Traffic3D is tested and supported on 64-bit Windows and Linux.

## Getting started

To get the Traffic3D source code, from the command line, run:

```sh
git clone git@gitlab.com:traffic3d/traffic3d.git
cd traffic3d
```

## Documentation

All documents should be in [Markdown format](https://about.gitlab.com/handbook/product/technical-writing/markdown-guide/) in a directory called `docs`.
Images should go in a `figures` subdirectory.

All Markdown documents should follow the rule of **one sentence per line** (i.e. a line break should follow every full stop).
This makes it *much* easier to view `git diff`s and review merge requests.

### Ensuring that your Markdown syntax is valid

To make sure that your Markdown is valid, please use [the mdl Markdown lint](https://github.com/markdownlint/markdownlint).

To install the lint on Debian-like machines, use the [Rubygems](https://rubygems.org/) package manager:

```sh
sudo apt-get install gem
sudo gem install mdl
```

Because we keep each sentence on a separate line, you will want to suppress spurious `MD013 Line length` reports by configuring `mdl`.
The file [.mdl.rb](/.mdl.rb) contains styles that deal with `MD013` and other tweaks we want to make to the lint.
To use the style configuration, pass it as a parameter to `mdl` on the command line:

```sh
mdl -s .mdl.rb DOCUMENT.md
```

If you want to run `mdl` from your IDE or editor, you will either need to configure it, or find a plugin, such as [this one for Sublime Text](https://github.com/SublimeLinter/SublimeLinter-mdl).

### Setting up a git hook

This repository provides a [git hook](https://githooks.com/) that will run `mdl` each time the developer commits their code, and refuse to perform the commit if the changes do not pass the lint.

To install the hook, first ensure that you have `mdl` installed correctly (see above).
Next, run the hook install script:

```sh
./bin/create-hook-symlinks
```

## Installation and set-up

The project runs on Unity 2018.3.11f1 and up.
Download the latest of Unity from the following link: [https://unity3d.com/get-unity/download/](https://unity3d.com/get-unity/download/)
Or download Unity 2018.3.11f1 from the following link: [https://unity3d.com/get-unity/download/archive](https://unity3d.com/get-unity/download/archive)

Use a preferred C# IDE or download Visual Studio using the following link: [https://visualstudio.microsoft.com/vs/](https://visualstudio.microsoft.com/vs/)

## Unity activation for the CI/CD environment

Install [docker](https://www.docker.com/) on your development machine, for example on Ubuntu 20.04:

```sh
sudo apt-get install docker.io
```

Pull the `gableroux/unity3d` docker image and run Bash inside, passing your unity username and password in to the environment:

```sh
UNITY_VERSION=2019.3.7f1
docker run -it --rm \
-e "UNITY_USERNAME=username@example.com" \
-e "UNITY_PASSWORD=example_password" \
-e "TEST_PLATFORM=linux" \
-e "WORKDIR=/root/project" \
-v "$(pwd):/root/project" \
gableroux/unity3d:$UNITY_VERSION \
bash
```

_hint: you should write this to a shell script and execute the shell script so you don't have your credentials stored in your bash history_.
Also make sure you use your Unity3d _email address_ for `UNITY_USERNAME` environment variable.

If your password contains a `!`, you can escape it like this (`example_pass!word`):

```sh
...
-e "UNITY_PASSWORD=example_pass"'!'"word" \
...
```

In the Docker container, run Unity once like this, it will try to activate the license:

```sh
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
/opt/Unity/Editor/Unity \
-logFile /dev/stdout \
-batchmode \
-username "$UNITY_USERNAME" -password "$UNITY_PASSWORD"
```

Wait for output that looks like this:

```sh
LICENSE SYSTEM [2017723 8:6:38] Posting <?xml version="1.0" encoding="UTF-8"?><root><SystemInfo><IsoCode>en</IsoCode><UserName>[...]
```

If you get the following error:

> Can't activate unity: No sufficient permissions while processing request HTTP error code 401

Make sure your credentials are valid.
You may try to disable 2FA in your account and try again.
Once done, you should enable 2FA again for security reasons.
See [this issue](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/-/issues/11) from the Unity3d [CI example repository](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/) for more details.

Next:

1. Copy the XML content and save in a file called `unity3d.alf`.
1. Open [https://license.unity3d.com/manual](https://license.unity3d.com/manual) and answer the questions.
1. Upload `unity3d.alf` for manual activation.
1. Download `Unity_v2018.x.ulf` (`Unity_v2019.x.ulf` for 2019 versions).
1. Copy the content of `Unity_v2018.x.ulf` license file to your CI's environment variable `UNITY_LICENSE_CONTENT`.

Note: if you are doing this on windows your [line endings will be wrong as explained here](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/5#note_95831816).
[`.gitlab-ci.yml`](.gitlab-ci.yml) solves this by removing `\r` character from the environment variable so you do not need to edit the license files manually.

[`.gitlab-ci.yml`](.gitlab-ci.yml) will then place the `UNITY_LICENSE_CONTENT` to the right place before running tests or creating the builds.

## Density Measurements

On new scenes, the density measure points need setting up to allow for correct density per km calculations.
To set these points up, on the paths, select the node that needs to be used as the density measure point (normally just after exiting a junction) and add the `DensityMeasurePoint.cs` script to the node.
Then add a `BoxCollider` to the node, check `Is Trigger` and resize where vehicles on that path will pass through the box.

## Further documentation

Detailed documentation, including how to use the Unity editor, how to extend Traffic3D with new assets and scenes, can be found in the [Traffic3D documentation](https://traffic3d.org).
