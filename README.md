# FolderSynchronization
Synchronization of two folders
 
FolderSynchronization is a command-line tool developed using .NET 7 that synchronizes two folders - a source folder and a replica folder. The tool maintains an identical copy of the content from the source folder in the replica folder. Synchronization is one-way, ensuring that the content of the replica folder matches the content of the source folder.

Usage
Clone the Repository:

Clone this repository to your local machine using the following command:

bash
Copy code
git clone https://github.com/your-username/FolderSynchronization.git
Build the Program:

Navigate to the project's root directory and build the program using the .NET CLI:

bash
cd FolderSynchronization
dotnet build

Run the Program:

Run the program with the following command, providing the required parameters:
dotnet run -- SourceFolderPath=<source_path> ReplicaFolderPath=<replica_path> LogFolderPath=<log_path> SyncInterval=<sync_interval>

Replace <source_path>, <replica_path>, <log_path>, and <sync_interval> with the appropriate values.

SourceFolderPath: The full path to the source folder.
ReplicaFolderPath: The full path to the replica folder.
LogFolderPath: The full path to the folder where log files will be stored.
SyncInterval: The synchronization interval in milliseconds.
