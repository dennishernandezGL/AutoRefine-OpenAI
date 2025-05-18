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
        this.message = message.trim();
        this.context = {
            componentName: (context.componentName || '').trim(),
            loggerUser: (context.loggerUser || '').trim(),
            environment: (context.environment || '').trim(),
            instanceIdentifier: (context.instanceIdentifier || '').trim(),
        };
        this.object = this.safeStringify(data);
    }

    private safeStringify(data: object): string {
        try {
            return JSON.stringify(data);
        } catch (error) {
            console.error('Failed to stringify object:', error);
            return '{}';
        }
    }
}