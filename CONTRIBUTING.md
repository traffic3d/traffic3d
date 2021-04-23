# Contributing to the Traffic 3D project

[[_TOC_]]

---

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

### Using Git with Unity

Git is the industry-standard distributed version control system, but Unity projects are not well suited for use with Git.
Unity produces large numbers of auto-generated files, which must be committed, but are not intended to be read by developers (and so should not be reviewed).
This includes `.unity`, `.meta`, and `.prefab` files.
This means that you may often find you need to resolve [merge conflicts](https://www.atlassian.com/git/tutorials/using-branches/merge-conflicts) for files which were never intended to be understood by developers.
Unity also produces _large_ files, which can make operations like `clone` considerably slower than you might expect.

Before you go much further, please read through [How to Git with Unity](https://thoughtbot.com/blog/how-to-git-with-unity) and consider using a [Unity-specific merge tool](https://docs.unity3d.com/Manual/SmartMerge.html)

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

### Setting up a Git hook

This repository provides a [Git hook](https://githooks.com/) that will run `mdl` each time the developer commits their code, and refuse to perform the commit if the changes do not pass the lint.

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

## Traffic3D Geometry

Please note that Traffic3D uses a coordinate system that assumes the earth is flat.

## Creating a new release

To create a new release of Traffic3D, follow these steps:

1. Create an MR with the version number in the title, merging `develop->master`. Remember that @bc-bot cannot force-push to the `develop` branch (because it is protected) so the MR has to be merged manually.
1. Assign it for review / merge to someone who has `Maintainer` rights or above.
1. Go to the [releases page](https://gitlab.com/traffic3d/traffic3d/-/releases) and create a new release [here](https://gitlab.com/traffic3d/traffic3d/-/tags/new).
1. From the release page, download the `.zip` file containing the source code for the release.
1. Go to the [Zenodo page](https://zenodo.org/record/3968432) for the Traffic3D software.
    1. Click on **New version**.
    1. Delete the `.zip` file from the previous version.
    1. Upload the new `.zip` file that you downloaded from the releases page.
    1. Bump the version number.
    1. Click **Save**.
    1. Click **Publish**.
1. The links on the website to download the built versions of Traffic3D always go to the latest build on `master`, and the Zenodo badges go to the multi-version DOI page, so neither of these need to be updated.
1. When the website MR is merged, push a tag for the next version of the website.
1. Go to the admin page on [readthedocs.org](https://readthedocs.org/projects/traffic3d/) and activate the tag that you just pushed.
1. Raise one final MR, to add a link to the activated tag on the navigation bar on the site.

## Further documentation

* [How to Git with Unity](https://thoughtbot.com/blog/how-to-git-with-unity)
* [Meld merge tool](https://meldmerge.org/)
* [Traffic3D user documentation](https://traffic3d.org) including how to use the Unity editor, how to extend Traffic3D with new assets and scenes.
* [Unity smart merge](https://docs.unity3d.com/Manual/SmartMerge.html)
