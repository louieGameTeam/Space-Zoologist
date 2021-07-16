mergeInto(LibraryManager.library, {

  SendCurrentLevelToWeb: function(levelName){
  
  
	ReactUnityWebGL.SendCurrentLevelToWeb(Pointer_stringify(levelName));
  },

  ToggleJournal: function(){

	ReactUnityWebGL.ToggleJournal();
  },



  ToggleCatalog: function(){

	ReactUnityWebGL.ToggleCatalog();
  }

});