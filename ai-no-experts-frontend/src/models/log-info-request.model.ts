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
        data: Record<string, unknown> = {}
      ) {
        this.message = message;
        this.context = {
          componentName: context.componentName?.trim() || '',
          loggerUser: context.loggerUser?.trim() || '',
          environment: context.environment?.trim() || '',
          instanceIdentifier: context.instanceIdentifier?.trim() || '',
        };
        this.object = JSON.stringify(data);
      }
}