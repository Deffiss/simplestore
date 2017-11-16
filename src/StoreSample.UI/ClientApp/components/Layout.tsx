import * as React from 'react';
import NavMenu from './NavMenu';

export class Layout extends React.Component<{}, {}> {
    public render() {
        return <div className='container-fluid'>
            <div className='row'>
                <div className='col-sm-2'>
                    <NavMenu />
                </div>
                <div className='col-sm-10' style={{width: 'calc(80%)', float: 'right', position: 'initial'}}>
                    { this.props.children }
                </div>
            </div>
        </div>;
    }
}
