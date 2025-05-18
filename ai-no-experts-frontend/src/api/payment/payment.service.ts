import axios from '../axios';

import type { Payment } from '../../models/payment.model';
import { LogInfoRequest } from '../../models/log-info-request.model';

export const submitPayment = async (data: Payment) => {
  try {
    const logInfoRequest = new LogInfoRequest(
      'Logging payment form user action',
      {
        componentName: 'PaymentForm',
        loggerUser: process.env.LOGGER_USER || 'defaultUser',
        environment: process.env.NODE_ENV || 'development',
        instanceIdentifier: process.env.INSTANCE_ID || 'defaultInstance'
      },
      data
    );
    
    const response = await axios.post(process.env.LOGGING_SERVICE_URL || 'https://secure.service/api/log/LogInfo', logInfoRequest);
  
    return {
      statusCode: response.status,
      data: response.data,
    };
  } catch (error: any) {
    console.error('submitPayment error:', error);
    
    return {
      statusCode: error?.response?.status || 500,
      data: error?.response?.data || { error: 'Unknown error occurred' },
    };
  }  
};