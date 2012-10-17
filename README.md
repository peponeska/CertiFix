# CertiFix

## Overview

CertiFix has been created to help managing user certificates in personal or corporate enviroment.
It is written in c# in Visual Studio 2012 and it is built on .NET Framework 4.0. 

## Features

CertiFix offers two actions
+ notifies user about expiring certificates in CurrentUser\My certificate store
+ notifies domain user about local certificates that are not in AD and are newer that certificates in AD and offers to add these certificates to AD

## Usage

CertiFix is published using ClickOnce technology. Published versions reside in install directory. Application checks for new versions at startup.

## Referenced projects

CertiFix uses a popup notification window library by Simon B. which is licensed under CPOL licence (http://www.codeproject.com/Articles/277584/Notification-Window)