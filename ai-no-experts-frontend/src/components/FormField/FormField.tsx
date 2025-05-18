import { FormControl, InputAdornment, TextField } from '@mui/material';
import { Field, ErrorMessage } from 'formik';
import { type FunctionComponent } from 'react';
import React from 'react';

const FormField: FunctionComponent<FormFieldProps> = ({
    endAdornment,    
    fullWidth = true,
    label = '',
    name = '',
    startAdornment,
    type = 'text',
    error = false,
    ...props
}) => {
    React.useEffect(() => {
        console.log('FormField rendered:', { name, label, type });
        // Log form interactions here or connect to a logging service
    }, [name, label, type]);

    return (
        <FormControl fullWidth margin="normal">
            <Field
                as={TextField}
                fullWidth
                id={name}
                name={name}
                label={label}
                type={type}
                error={error}
                InputProps={{
                    startAdornment: startAdornment ? <InputAdornment position="start">{startAdornment}</InputAdornment> : null,
                    endAdornment: endAdornment ? <InputAdornment position="start">{endAdornment}</InputAdornment> : null,
                }}
                {...props}
            />
            <ErrorMessage className='form-error' name={name} component="p" />
        </FormControl>
    );
}

type FormFieldProps = {
    endAdornment?: string;
    error?: boolean;
    fullWidth?: boolean,
    label: string;
    name: string;
    startAdornment?: string;
    type?: string;
    [key: string]: any;
}

export default FormField;