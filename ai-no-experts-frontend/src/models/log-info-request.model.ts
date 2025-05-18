export class LogInfoRequest {    
    context: {
        componentName: string;
        loggerUser: string;
        environment: string;
        instanceIdentifier: string;
    };
    message: string;
    object: string;
    
    constructor (
        message: string = '',
        context: {
          componentName?: string;
          loggerUser?: string;
          environment?: string;
          instanceIdentifier?: string;
        } = {},
        data: object = {}
      ) {
        this.message = message;
        this.context = {
          componentName: this.validateString(context.componentName),
          loggerUser: this.validateString(context.loggerUser),
          environment: this.validateString(context.environment),
          instanceIdentifier: this.validateString(context.instanceIdentifier),
        };
        this.object = JSON.stringify(data);
      }

      private validateString(value?: string): string {
        if (value === null || value === undefined) {
          return '';
        }
        return value;
      }
}