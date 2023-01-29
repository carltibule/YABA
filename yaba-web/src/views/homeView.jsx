import React from "react";
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

export function HomeView(props) {
    return(
        <React.Fragment>
            <div style={{flexGrow: 1}}>
                <Container>
                    <Row>
                        <Col xs="9">
                            <h1>This is the Home View</h1>
                        </Col>
                    </Row>
                </Container> 
            </div>
        </React.Fragment>
    );
}