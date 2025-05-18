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
          componentName: context.componentName || '',
          loggerUser: context.loggerUser || '',
          environment: context.environment || '',
          instanceIdentifier: context.instanceIdentifier || '',
        };
        // Ensure data to be serialized does not pose security risks
        this.object = sanitizeOrSecureJSONStringify(data);
      }
}

// A placeholder example function for potential sanitization
function sanitizeOrSecureJSONStringify(data: object): string {
    // Implement sanitization or security measures here
    // For now, simply returning JSON.stringify as a placeholder
    return JSON.stringify(data);
}