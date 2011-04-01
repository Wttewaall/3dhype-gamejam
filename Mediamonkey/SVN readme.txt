Using External Version Control Systems with Unity

Unity offers an Asset Server add-on product for easy integrated versioning of your projects. If you for some reason are not able use the Unity Asset Server, it is possible to store your project in any other version control system, such as Subversion, Perforce or Bazaar, although this requires some manual initial setup of your project and moving and renaming of assets has to be performed using your version control client and not inside Unity.

External Version Control is a Unity Pro feature.

Before checking your project in, you have to tell Unity to modify the project structure slightly to make it compatible with storing assets in an external version control system. This is done by selecting Edit->Project Settings->Editor in the application menu and enabling External Version Control support by clicking the Enable button. This will create a text file for every asset in the Assets directory containing the necessary bookkeeping information required by Unity. The files will have a .meta file extension with the first part being the full file name of the asset it is associated with. When moving or renaming assets in the version control system, make sure you also move or rename the .meta file accordingly.

When checking the project into a version control system, you should at least add the Assets directory to the system. In case you want to track project and build settings as well you also need to add the Library folder and these files inside the folder:

    * EditorBuildSettings.asset
    * InputManager.asset
    * ProjectSettings.asset
    * QualitySettings.asset
    * TagManager.asset
    * TimeManager.asset
    * AudioManager.asset
    * DynamicsManager.asset
    * NetworkManager.asset 

Do not add any other files or directories located inside the Library directory. When creating new assets, make sure both the asset itself and the associated .meta file is added to version control. 