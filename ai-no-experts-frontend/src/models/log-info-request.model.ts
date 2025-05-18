export class LogInfoRequest {
    context: {
        componentName: string;
        loggerUser: string;
        environment: string;
        instanceIdentifier: string;
    };
    message: string;
    object: string;

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
            componentName: context.componentName?.trim() || 'unknown',
            loggerUser: context.loggerUser?.trim() || 'anonymous',
            environment: context.environment?.trim() || 'none',
            instanceIdentifier: context.instanceIdentifier?.trim() || 'none',
        };
        try {
            this.object = JSON.stringify(data);
        } catch (error) {
            console.error('Error stringifying object:', error);
            this.object = '{}';
        }
    }
}