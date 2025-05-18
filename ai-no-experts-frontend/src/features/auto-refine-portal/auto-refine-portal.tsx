import { Box, Container } from "@mui/material";
import { useState } from "react";
import { trackEvent } from "../../utils/analytics"; /* Assume this is a utility for event tracking */

// ... unchanged components ...

    const onPaymentFormSubmit = async (values: Payment) => {             
        trackEvent('PaymentForm Submission', { values }); // New line for tracking form submission

        try {
            setIsLoading(true);
            setErrorMessage('');

            const response = await submitPayment(values);
            
            if (response.statusCode === 200) {
                setSuccessMessage(response.data);
                trackEvent('Payment Successful', { data: response.data }); // Tracking success event
            } else {
                const errors = response.data?.errors;
                if (errors) {
                    const errorMessages = Object.entries(errors)
                        .map(([field, messages]: any) => `${field}: ${messages.join(', ')}`)
                        .join(' | ');

                    setErrorMessage(errorMessages);
                    trackEvent('Payment Failed', { errorMessages }); // Tracking failure
                } else {
                    const unknownError = "An unknown error occurred.";
                    setErrorMessage(unknownError);
                    trackEvent('Payment Failed', { error: unknownError }); // Tracking failure
                }
            }

            setIsLoading(false);
        } catch (error: any) {
            setIsLoading(false);
            console.error('Submission error:', error);
            setErrorMessage(`Error: ${error.message}`);
            trackEvent('Submission Error', { error: error.message }); // Enhanced error tracking
        }
    }