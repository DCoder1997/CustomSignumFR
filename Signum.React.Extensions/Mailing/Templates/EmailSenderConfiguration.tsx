import * as React from 'react'
import { ValueLine, EntityRepeater, EntityDetail } from '@framework/Lines'
import { TypeContext } from '@framework/TypeContext'
import { EmailSenderConfigurationEntity, SmtpNetworkDeliveryEmbedded, ClientCertificationFileEmbedded, SmtpEmbedded, ExchangeWebServiceEmbedded } from '../Signum.Entities.Mailing'
import { DoublePassword } from '../../Authorization/AuthClient'
import { Binding } from '@framework/Reflection'

export default function EmailSenderConfiguration(p: { ctx: TypeContext<EmailSenderConfigurationEntity> }) {
  const sc = p.ctx;

  return (
    <div>
      <ValueLine ctx={sc.subCtx(s => s.name)} />
      <EntityDetail ctx={sc.subCtx(s => s.defaultFrom)} />
      <EntityRepeater ctx={sc.subCtx(s => s.additionalRecipients)} />
      <EntityDetail ctx={sc.subCtx(s => s.sMTP)} getComponent={(smtp: TypeContext<SmtpEmbedded>) =>
        <div>
          <ValueLine ctx={smtp.subCtx(s => s.deliveryFormat)} />
          <ValueLine ctx={smtp.subCtx(s => s.deliveryMethod)} />
          <ValueLine ctx={smtp.subCtx(s => s.pickupDirectoryLocation)} />
          <EntityDetail ctx={smtp.subCtx(s => s.network)} getComponent={(net: TypeContext<SmtpNetworkDeliveryEmbedded>) =>
            <div>
              <ValueLine ctx={net.subCtx(s => s.port)} />
              <ValueLine ctx={net.subCtx(s => s.host)} />
              <ValueLine ctx={net.subCtx(s => s.useDefaultCredentials)} />
              <ValueLine ctx={net.subCtx(s => s.username)} />
              {!sc.readOnly &&
                <DoublePassword ctx={new TypeContext<string>(net, undefined, undefined as any, Binding.create(net.value, v => v.newPassword))} isNew={net.value.isNew} />}
              <ValueLine ctx={net.subCtx(s => s.enableSSL)} />
              <EntityRepeater ctx={net.subCtx(s => s.clientCertificationFiles)} getComponent={(cert: TypeContext<ClientCertificationFileEmbedded>) =>
                <div>
                  <ValueLine ctx={cert.subCtx(s => s.certFileType)} />
                  <ValueLine ctx={cert.subCtx(s => s.fullFilePath)} />
                </div>
              } />
            </div>
          } />
        </div>
      } />
      <EntityDetail ctx={sc.subCtx(s => s.exchange)} getComponent={(smtp: TypeContext<ExchangeWebServiceEmbedded>) =>
        <div>
          <ValueLine ctx={smtp.subCtx(s => s.exchangeVersion)} />
          <ValueLine ctx={smtp.subCtx(s => s.url)} />
          <ValueLine ctx={smtp.subCtx(s => s.useDefaultCredentials)} />
          <ValueLine ctx={smtp.subCtx(s => s.username)} />
          <ValueLine ctx={smtp.subCtx(s => s.password)} />
        </div>
      } />
    </div>
  );
}

