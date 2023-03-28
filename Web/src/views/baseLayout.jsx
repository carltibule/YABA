import React from 'react';
import Stack from 'react-bootstrap/Stack';

export function BaseLayout(props) {
    return (
        <React.Fragment>
            <Stack gap={3}>
                { props.header }
                { props.content }
                { props.footer }
            </Stack>
        </React.Fragment>
    );
}