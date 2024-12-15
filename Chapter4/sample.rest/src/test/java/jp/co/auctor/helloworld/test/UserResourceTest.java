package jp.co.auctor.helloworld.test;

import static org.junit.Assert.*;

import java.net.URISyntaxException;

import javax.servlet.http.HttpServletResponse;

import org.codehaus.jackson.map.ObjectMapper;
import org.jboss.logging.Logger;
import org.jboss.resteasy.core.Dispatcher;
import org.jboss.resteasy.mock.MockDispatcherFactory;
import org.jboss.resteasy.mock.MockHttpRequest;
import org.jboss.resteasy.mock.MockHttpResponse;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Assert;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.ApplicationContext;
import org.springframework.test.annotation.Rollback;
import org.springframework.test.context.ContextConfiguration;
import org.springframework.test.context.TestExecutionListeners;
import org.springframework.test.context.junit4.SpringJUnit4ClassRunner;
import org.springframework.test.context.support.DependencyInjectionTestExecutionListener;
import org.springframework.test.context.support.DirtiesContextTestExecutionListener;
import org.springframework.transaction.annotation.Transactional;

import com.github.springtestdbunit.DbUnitTestExecutionListener;
import com.github.springtestdbunit.TransactionDbUnitTestExecutionListener;
import com.github.springtestdbunit.annotation.DatabaseSetup;

import jp.co.auctor.helloworld.entity.Users;
import jp.co.auctor.helloworld.resource.UsersResource;

@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration(locations = "classpath:application-test-context.xml")
@TestExecutionListeners({ DependencyInjectionTestExecutionListener.class,
        DirtiesContextTestExecutionListener.class,
        TransactionDbUnitTestExecutionListener.class,
        DbUnitTestExecutionListener.class })
@DatabaseSetup("classpath:testdata/users.xml")
public class UserResourceTest {
    private Dispatcher dispatcher = MockDispatcherFactory.createDispatcher();

    /**
     * ロガー。
     * */
    Logger logger = Logger.getLogger(UserResourceTest.class);

    @Autowired
    ApplicationContext ctx;

    //@Autowired
    //BeanConfig beanConfig;

    UsersResource usersResource;

    @BeforeClass
    public static void execBeforeClass() {
    }

    @Before
    public void execBefore() {
        usersResource = ctx.getAutowireCapableBeanFactory().createBean(UsersResource.class);
        dispatcher.getRegistry().addSingletonResource(usersResource);
    }

    @Test
    public void userResourceGetUsersTest01() throws URISyntaxException {
        logger.info("★★★★★UserResource.getUsers　test 01 ★★★★★");
        logger.info("*** NO.1000 を検索　***");

        try {
            MockHttpRequest request = MockHttpRequest.get("/users/1000");
            MockHttpResponse response = new MockHttpResponse();

            dispatcher.invoke(request, response);

            String strJson = response.getContentAsString();
            ObjectMapper mapper = new ObjectMapper();
            Users bean = mapper.readValue(strJson, Users.class);

            logger.info("id:" + bean.getId() + " name:" + bean.getName() + " email: " + bean.getEmail());

            Assert.assertEquals(HttpServletResponse.SC_OK, response.getStatus());
        } catch (Exception e) {
            logger.error(e.toString());
            fail(e.toString()); //テスト失敗
        }
    }

    @Test
    public void userResourceGetUsersTest02() throws URISyntaxException {
        logger.info("★★★★★UserResource.getUsers　test 02★★★★★");
        logger.info("*** 全件検索　***");

        try {
            MockHttpRequest request = MockHttpRequest.get("/users");
            MockHttpResponse response = new MockHttpResponse();

            dispatcher.invoke(request, response);

            Assert.assertEquals(HttpServletResponse.SC_OK, response.getStatus());

            String strJson = response.getContentAsString();
            ObjectMapper mapper = new ObjectMapper();
            Users[] bean = mapper.readValue(strJson, Users[].class);

            for (int i = 0; i < bean.length; i++) {
                logger.info("id:" + bean[i].getId() + " name:" + bean[i].getName() + " email: " + bean[i].getEmail());
            }

            //件数を確認
            assertEquals(6, bean.length);

        } catch (Exception e) {
            logger.error(e.toString());
            fail(e.toString()); //テスト失敗
        }
    }

    @Test
    @Rollback(true)
    @Transactional
    public void userResourceGetUsersTest03() throws URISyntaxException, InterruptedException {
        logger.info("★★★★★UserResource.getUsers　test 03★★★★★");
        logger.info("*** NO.2000 を削除　***");

        try {

            MockHttpRequest request = MockHttpRequest.delete("/users/2000");
            MockHttpResponse response = new MockHttpResponse();

            dispatcher.invoke(request, response);

            Assert.assertEquals(HttpServletResponse.SC_NO_CONTENT, response.getStatus());

        } catch (Exception e) {
            logger.error(e.toString());
            fail(e.toString()); //テスト失敗
        }
    }

    @After
    public void execAfter() {
    }

    @AfterClass
    public static void execAfterClass() {
    }

}
