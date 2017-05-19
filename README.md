[![NuGet Version](http://img.shields.io/nuget/v/GeekLearning.Storage.svg?style=flat-square&label=NuGet:%20Abstractions)](https://www.nuget.org/packages/GeekLearning.Storage/)
[![NuGet Version](http://img.shields.io/nuget/v/GeekLearning.Storage.FileSystem.svg?style=flat-square&label=NuGet:%20FileSystem)](https://www.nuget.org/packages/GeekLearning.Storage.FileSystem/)
[![NuGet Version](http://img.shields.io/nuget/v/GeekLearning.Storage.Azure.svg?style=flat-square&label=NuGet:%20Azure%20Storage)](https://www.nuget.org/packages/GeekLearning.Storage.Azure/)
[![Build Status](https://geeklearning.visualstudio.com/_apis/public/build/definitions/f841b266-7595-4d01-9ee1-4864cf65aa73/27/badge)](#)

# Geek Learning Cloud Storage Abstraction

This library abstracts physical data storage in a way which allows you to transparently switch the underlying provider
by configuration.

## Features

* List files, with globbing support
* Read, Write, Delete files
* Public file url

## Providers

The library currently supports:
* Azure Blob Storage
* File System Storage

We don't support for Amazon S3, but it is one of our high priority objective.

## Getting Started

You can head to our introduction [blog post](http://geeklearning.io/dotnet-core-storage-cloud-or-file-system-storage-made-easy/), 
or to the [wiki](https://github.com/geeklearningio/gl-dotnet-storage/wiki).

