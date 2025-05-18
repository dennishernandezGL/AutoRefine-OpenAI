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
        this.message = this.sanitize(message);
        this.context = {
          componentName: this.sanitize(context.componentName) || '',
          loggerUser: this.sanitize(context.loggerUser) || '',
          environment: this.sanitize(context.environment) || '',
          instanceIdentifier: this.sanitize(context.instanceIdentifier) || '',
        };
        this.object = JSON.stringify(this.sanitizeObject(data));
      }
    
    private sanitize(input: string = ''): string {
        return input.replace(/[<>]/g, ''); // Simple sanitization for example purposes
    }

    private sanitizeObject(obj: object): object {
        return JSON.parse(JSON.stringify(obj, (key, value) => {
            return (typeof value === 'string') ? this.sanitize(value) : value;
        }));
    }
}