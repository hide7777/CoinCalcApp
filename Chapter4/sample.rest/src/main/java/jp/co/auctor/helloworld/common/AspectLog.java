package jp.co.auctor.helloworld.common;

import java.lang.reflect.Field;

import org.aspectj.lang.JoinPoint;
import org.aspectj.lang.annotation.After;
import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.annotation.Before;
import org.jboss.logging.Logger;
import org.springframework.beans.factory.config.BeanDefinition;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Component;

@Aspect
@Component
@Scope(BeanDefinition.SCOPE_PROTOTYPE)
public class AspectLog {
    //@Autowired
    //ApplicationContext ctx;
    //@Autowired
    //UsersResource usersResource;

    //@SuppressWarnings("restriction")
    @Before("execution(* jp.co.auctor.helloworld.service.UsersService.*(..))")
    private void beforeService(JoinPoint jp) throws Throwable {

        //String match = "jp.co.auctor.helloworld.resource.UsersResource";
        Logger logger = Logger.getLogger(AspectLog.class);
        try {
            logger.info("serviceBefore:" + jp.getSignature().getName());
            Object[] args = jp.getArgs();
            for (Object o : args) {
                logger.info("serviceBefore:" + o);
            }
/*
            Class caller = null;
            for (int i = 0; (caller = sun.reflect.Reflection.getCallerClass(i)) != null; ++i) {
                if (caller.getName().startsWith(match)) {
                    logger.info(caller.getName());
                    try {
                        BeanUtil.getProperty(caller, "uuidClass");

                        Object target = caller;
                        Class c = target.getClass();
                        Field f = c.getDeclaredField("uuidClass");
                        f.setAccessible(true);
                        UuidClass uuidClass = (UuidClass) f.get(target);
                        logger.info("serviceBefore:UUID=" + uuidClass.getUuid());
                    } catch (Exception e) {
                        logger.info("uuid fot found");
                    }
                }
            }
            */
            /*
            StackTraceElement[] ste = Thread.currentThread().getStackTrace();
            for (int i = 0; i < ste.length; i++) {
                //logger.info(sun.reflect.Reflection.getCallerClass().getName());

                if ((ste[i].getClassName().toString()).startsWith(match)) {
                    logger.info("serviceAfter");
                }
            }
            logger.info("serviceAfter");
            */
            //Object target = jp.getTarget();
            //Class c = target.getClass();
            //Field f = c.getDeclaredField("uuidClass");
            //f.setAccessible(true);
            //UuidClass uuidClass = (UuidClass) f.get(target);
            //logger.info("serviceBefore:UUID=" + uuidClass.getUuid());

            //String[] strArr = ctx.getBeanDefinitionNames();
            //for (String string : strArr) {
            //    System.out.println(string);
            //}

            //StackTraceElement[] ste = Thread.currentThread().getStackTrace();
            //for (int i = 0; i < ste.length; i++) {
            //  if ((ste[i].getClassName().toString()).startsWith(match)) {
            //      System.out.println(ste[i].getClassName()); // クラス名を取得

            //UsersResource target = new UsersResource();
            //AutowireCapableBeanFactory factory = ctx.getAutowireCapableBeanFactory();
            //factory.autowireBean(target);
            //factory.initializeBean(target, "usersResource");

            //Object target = ctx.getAutowireCapableBeanFactory().getBean(UsersResource.class);
            //Class<?> c = target.getClass();
            //Object target = usersResource;
            //Class<?> c = target.getClass();
            //Field f = c.getDeclaredField("uuidClass");
            //f.setAccessible(true);
            //UuidClass uuidClass = (UuidClass) f.get(target);
            //logger.info("serviceBefore:UUID=" + uuidClass.getUuid());
            //}
            // }
        } catch (Exception e) {
            logger.info(e.toString());
        }

    }

    @Before("execution(* jp.co.auctor.helloworld.resource.UsersResource.*(..))")
    private void before(JoinPoint jp) {
        Logger logger = Logger.getLogger(AspectLog.class);
        try {
            logger.info("resourceBefore:" + jp.getSignature().getName());
            Object[] args = jp.getArgs();
            for (Object o : args) {
                logger.info("resourceBefore:" + o);
            }
            Object target = jp.getTarget();
            Class<? extends Object> c = target.getClass();
            Field f = c.getDeclaredField("uuidClass");
            f.setAccessible(true);
            UuidClass uuidClass = (UuidClass) f.get(target);
            logger.info("resourceBefore:UUID=" + uuidClass.getUuid());
        } catch (Exception e) {
            logger.info(e.toString());
        }

    }

    @After("execution(* jp.co.auctor.helloworld.resource.UsersResource.*(..))")
    private void after(JoinPoint jp) {
        Logger logger = Logger.getLogger(AspectLog.class);
        try {
            logger.info("resourceAfter:" + jp.getSignature().getName());
            Object target = jp.getTarget();
            Class<? extends Object> c = target.getClass();
            Field f = c.getDeclaredField("uuidClass");
            f.setAccessible(true);
            UuidClass uuidClass = (UuidClass) f.get(target);
            logger.info("resourceAfter:UUID=" + uuidClass.getUuid());
        } catch (Exception e) {
            logger.info(e.toString());
        }

    }

}
