import axios from '../axios';

import type { Payment } from '../../models/payment.model';

export const submitPayment = async (data: Payment) => {
  try {
    const response = await axios.post('/form-ai/recommend', {
      logs: [{ values: data }],
    });
  
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