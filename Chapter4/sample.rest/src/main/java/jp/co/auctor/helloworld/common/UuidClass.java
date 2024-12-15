package jp.co.auctor.helloworld.common;

import java.util.UUID;

import org.springframework.beans.factory.config.BeanDefinition;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Component;

@Component
@Scope(BeanDefinition.SCOPE_PROTOTYPE)
public class UuidClass {

      private UUID uuid;

        public UuidClass() {
            this.uuid = UUID.randomUUID();
        }

        public UUID getUuid() {
            return this.uuid;
        }

}
