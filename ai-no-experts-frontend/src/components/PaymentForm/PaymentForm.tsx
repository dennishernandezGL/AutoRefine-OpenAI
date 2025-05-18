cvv: Yup.string().matches(/^[0-9]{3,4}$/, 'Invalid CVV').required('CVV is required'),
ssn: Yup.string().matches(/^[0-9]{3}-[0-9]{2}-[0-9]{4}$/, 'Invalid SSN'),
