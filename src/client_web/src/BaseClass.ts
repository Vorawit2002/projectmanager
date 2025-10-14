export class BaseClass {
    /**
     * authorization token value
     */

  
    constructor() {}
  
      protected transformOptions(options: any) { 
     
      if (localStorage.getItem('TOKEN_KEY')) {
        options.headers["Authorization"] = "bearer " + localStorage.getItem('TOKEN_KEY')
      } else {
        console.warn("Authorization token have not been set please authorize first.");
      }
          return Promise.resolve(options); 
      } 
  }