#Introduction

AppLaunchMenu allows for multiple applications to be streamed in a single session using Citrix Workspace, Amazon AppStream, Azure RemoteApp, or any other remote application virtualization platform. Virtual delivery of multiple applications with different environments and configurations is possible while sharing a common cloud-based runtime environment. Where applications need to "talk" with each other, this approach ensures that applications are always started on the same remote session.

Instead of publishing individual applications for streaming, an administrator publishes the AppLaunchMenu to an account/tenant (i.e., Production, Development & Testing), and potentially a group of machines within, on which the applications run. The AppLaunchMenu is the application that starts on the remote session, and once the session is established, allows an end-user to start additional applications configured on the menu. This allows a user to have a remote session that they then have full control over as a compute space, starting applications and interacting with them, without having to return to re-stream when an application needs to be switched or re-started. This is especially useful when users are switching between different environments for the same application (i.e., Development, Test, User Acceptance Testing).

AppLaunchMenu is configured using a LaunchMenu file which is written using XML. This file is stored on a common file share that is accessible by the end-user, and the accounts/tenants on which it is published. An administrator creates the LaunchMenu file and configures the published AppLaunchMenu's command-line to use this file on streaming start. The LaunchMenu file contains within it all the applications that an end-user can start and allows for each application to have a specific or shared environment and configuration.

AppLaunchMenu is written in Visual Studio using the C#, .NET, and WinUI. WinUI is now the primary UI stack for the Windows App SDK, decoupling the UI framework from the Windows Operating System to allow for faster iteration and backward compatibility. It is designed to provide a modern user interface layer for Windows applications, offering Fluent controls and styles. 

#Using AppLaunchMenu

AppLaunchMenu is a simple application, all based around the LaunchMenu file. The LaunchMenu file defines Menus, each represented by a tab along the top. Menus allow for the separation of applications into larger groups (e.g. Applications vs. Development Tools). By default, the first Menu in the LaunchMenu file is displayed. In each Menu are Folders and Applications. A Folder is not required but is a nice way to organize Applications into groups (e.g. Development Environments, Test Environments, etc.). An Application is the published program that the user can double-click, or select and click the Launch button, to start. The Application software must be installed on the remote instance prior to being started, otherwise an error message will indicate that the Application was not found. For an end-user, AppLaunchMenu is as simple as that, and fairly intuitive to use.

For the administrator configuring AppLaunchMenu, there are more capabilities available to configure each available application. It is possible to configure an Environment, and Variables for each application, or shared across multiple applications. An Environment can exist for all Menus, a specific Menu, a specific Folder, or a specific Application. At each level, everything underneath will share the Environment configured above. Within each Environment are several Variables that contain a specific setting to be used either at the Process level, or as a value in a Configuration file, each time the application is started.

Advanced features such as configuring Data Center grouping, Network Drives for automatic mapping on startup, and Scripts to extend LaunchMenu file configuration capabilities are also available. Finally, Application Services which represent Windows Services, can be configured to be installed and uninstalled directly from the AppLaunchMenu or from the command-line allows for a single location for all application configuration, both client and server.

Please see the Detailed description of the LaunchMenu file for more details.

#Contributing

AppLaunchMenu is licensed under the [MIT License]. Contributions can be made; however, they must confirm to the purpose and simplistic design of the application to be accepted.
