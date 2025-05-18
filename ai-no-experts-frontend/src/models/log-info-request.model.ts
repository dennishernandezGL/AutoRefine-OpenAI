export class LogInfoRequest {
    context: {
        componentName: string;
        loggerUser: string;
        environment: string;
        instanceIdentifier: string;
    } = {
        componentName: '',
        loggerUser: '',
        environment: '',
        instanceIdentifier: ''
    };
    message: string = '';
    object: string = '';

    constructor(
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
        this.object = JSON.stringify(data);
    }
}